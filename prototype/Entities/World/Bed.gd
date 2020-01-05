extends "res://Entities/World/TracksPlayer.gd"

signal end_day

func _ready():
	self.connect("player_interacted", self, "_on_player_interacted")

func _on_player_interacted():
	Globals.player.freeze()
	self.emit_signal("end_day")
