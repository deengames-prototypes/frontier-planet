extends "res://Entities/World/TracksPlayer.gd"

signal end_day

func _ready():
	self.connect("player_interacted", self, "_on_player_interacted")
	$DialogPanel.set_text("Press space to sleep")

func _on_player_interacted():
	Globals.player.freeze()
	self.emit_signal("end_day")

func _on_Bed_body_entered(body):
	if body.name == "MapPlayer":
		$DialogPanel.visible = true

func _on_Bed_body_exited(body):
	if body.name == "MapPlayer":
		$DialogPanel.visible = false
