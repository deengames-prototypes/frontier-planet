extends "res://Entities/World/TracksPlayer.gd"

func _ready():
	self.connect("player_interacted", self, "_on_player_interacted")

func _on_player_interacted():
	# save player position and GO FISHIN~!
	Globals.player_position = _player.position
	get_tree().change_scene("res://Scenes/Fishing.tscn")