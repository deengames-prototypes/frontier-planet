extends Area2D

const Home = preload("res://Scenes/Home.tscn")
const SceneManagement = preload("res://Scripts/SceneManagement.gd")

const FADE_TIME_SECONDS = 0.5

export var damage_per_second = 10
var _player_in_range = false

func _on_body_entered(body):
	if body.name == "MapPlayer":
		_player_in_range = true

func _on_body_exited(body):
	if body.name == "MapPlayer" and _player_in_range:
		_player_in_range = false

func _process(delta):
	if _player_in_range:
		Globals.player_health -= (damage_per_second * delta)
		if Globals.player_health <= 0 and !Globals.player._frozen:
			
			Globals.player.freeze()
			Globals.player_health = 0
			
			var canvas_modulate = CanvasModulate.new()
			add_child(canvas_modulate)
			var tween = Tween.new()
			tween.interpolate_property(canvas_modulate, "color", Color.white, Color.black, FADE_TIME_SECONDS, Tween.TRANS_LINEAR, Tween.EASE_IN_OUT)
			tween.connect("tween_completed", self, "_fade_in")
			add_child(tween)
			tween.start()

func _fade_in(a, b):
	var home_instance = Home.instance()
	SceneManagement.change_map_to(get_tree(), home_instance, "Bed")
	home_instance._fade_in(a, b) # pass time and fade in