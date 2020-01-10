extends "res://Entities/World/TracksPlayer.gd"

func _ready():
	self.connect("player_arrived", self, "_i_can_has_omelet")
	self.connect("player_departed", self, "_bye_bye")

func _i_can_has_omelet():
	if Globals.quest_indicies["Hungry Hippo"] == 0:
		if not $DialogPanel.visible:
			
			var found_omelet = null
			for item in Globals.player_inventory:
				if item.find("omelet") > -1:
					found_omelet = item
					break
			
			if found_omelet != null:
				$DialogPanel.set_text("Oh hey, a delicious omelet, jazakumullahu khayran katheeran!!!")
				Globals.player_inventory.erase(found_omelet)
				Globals.gain_community_points(10)
				Globals.quest_indicies["Hungry Hippo"] = 1
			else:
				$DialogPanel.set_text("Salams bro! Feeling a bit peckish, got anything delish?")
				
			$DialogPanel.visible = true
		else:
			$DialogPanel.visible = false
	else:
		$DialogPanel.set_text("Jazakumullahu khayran katheeran for the AMAZING omelet!!!")
		$DialogPanel.visible = true

func _bye_bye():
	$DialogPanel.visible = false