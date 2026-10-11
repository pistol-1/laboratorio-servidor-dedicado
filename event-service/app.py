import threading
import time

from flask import Flask, jsonify, request

app = Flask(__name__)

games = {}
games_lock = threading.Lock()


def get_or_create_game(game_id):
    return games.setdefault(game_id, {"events": [], "sequence_by_event_id": {}})


def validate_event_body(body):
    if not isinstance(body, dict):
        return "El cuerpo debe ser un objeto JSON"

    for field in ("playerId", "type"):
        if not isinstance(body.get(field), str) or not body[field].strip():
            return f"Falta el campo {field}"

    if not isinstance(body.get("payload"), dict):
        return "Falta el campo payload"

    return None


def parse_since(raw_value):
    if raw_value is None:
        return 0

    try:
        since = int(raw_value)
    except ValueError:
        return None

    return since if since >= 0 else None


@app.post("/games/<game_id>/events")
def register_event(game_id):
    body = request.get_json(silent=True)
    error = validate_event_body(body)

    if error:
        return jsonify({"message": error}), 422

    event_id = body.get("eventId")

    with games_lock:
        game = get_or_create_game(game_id)

        if event_id in game["sequence_by_event_id"]:
            return jsonify({"seq": game["sequence_by_event_id"][event_id]}), 200

        event = {
            "seq": len(game["events"]) + 1,
            "playerId": body["playerId"],
            "type": body["type"],
            "payload": body["payload"],
            "timestamp": time.time(),
        }
        game["events"].append(event)

        if event_id:
            game["sequence_by_event_id"][event_id] = event["seq"]

    return jsonify({"seq": event["seq"], "timestamp": event["timestamp"]}), 201


@app.get("/games/<game_id>/events")
def list_events(game_id):
    since = parse_since(request.args.get("since"))

    if since is None:
        return jsonify({"message": "since debe ser un entero mayor o igual a 0"}), 422

    with games_lock:
        events = games.get(game_id, {"events": []})["events"]
        new_events = events[since:]
        last_seq = len(events)

    return jsonify({"events": new_events, "lastSeq": last_seq}), 200
