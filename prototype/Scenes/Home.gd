extends "res://Scenes/Map.gd"

const FADE_TIME_SECONDS = 0.3

# TODO: could probably be preload(...) => int
const FOREST_SPAWN_PROBABILITIES = {
	"Morel": 20,
	"Mushroom": 50,
	"Herb": 30
}

const FOREST_SPOT_PROBABILITY = 70 # 40 = 40% chance to spawn at each spot

func _on_Bed_end_day():
	var tween = Tween.new()
	tween.interpolate_property($CanvasModulate, "color", Color.white, Color.black, FADE_TIME_SECONDS, Tween.TRANS_LINEAR, Tween.EASE_IN_OUT)
	tween.connect("tween_completed", self, "_fade_in")
	add_child(tween)
	tween.start()
	
func _fade_in(a, b):
	yield(get_tree().create_timer(1), "timeout")

	self.start_new_day()
	
	var tween = Tween.new()
	tween.interpolate_property($CanvasModulate, "color", Color.black, Color.white, FADE_TIME_SECONDS, Tween.TRANS_LINEAR, Tween.EASE_IN_OUT)
	add_child(tween)
	tween.start()
	
	tween.connect("tween_completed", self, "_unfreeze_player")

func _unfreeze_player(a, b):
	Globals.player.unfreeze()
	
func start_new_day():
	#### region of: day rolling over
	# TODO: event bus and everyone who cares subscribes
	###
	$Chicken.on_end_day()
	
	Globals.forest_items = []
	for i in range(Globals.FOREST_ITEMS_TO_SPAWN):
		if randi() % 100 <= FOREST_SPOT_PROBABILITY:
			# SPAWN MORE OVERLORDS!
			# TODO: could be more dynamic
			var spawn_what = randi() % 100
			if spawn_what <= FOREST_SPAWN_PROBABILITIES["Morel"]:
				Globals.forest_items.append("Morel")
			elif spawn_what <= FOREST_SPAWN_PROBABILITIES["Morel"] + FOREST_SPAWN_PROBABILITIES["Mushroom"]:
				Globals.forest_items.append("Mushroom")
			else:
				Globals.forest_items.append("Herb")
	print("Today: " + str(Globals.forest_items))