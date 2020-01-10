extends "res://Entities/World/TracksPlayer.gd"

func _ready():
	self.visible = Globals.community_level >= 3
	if self.visible:
		self.connect("player_interacted", self, "_on_use_stove")

func _process(delta):
	$Label.visible = _player_in_range
	if not $Label.visible:
		$Label.text = "Stove"
	
func _on_use_stove():
	$Label.text = "Used the stove."