extends CanvasLayer

func _ready():
	$Label2/HealthBar.max_value = Globals.MAX_HEALTH

func _process(delta):
	# Overkill but keeps it up-to-date
	# TODO: have an event bus and hook to that on points/level-up
	$Label.text = "Community level: " + str(Globals.community_level)
	$Label.text += "\nXP: " + str(Globals.community_xp) + "/" + str(Globals.COMMUNITY_XP_TO_LEVEL_UP)
	
	# Ditto: event bus for on-damage-update-healthbar
	$Label2/HealthBar.value = Globals.player_health