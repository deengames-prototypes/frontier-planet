extends "res://Scenes/Map.gd"

const FADE_TIME_SECONDS = 0.3

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