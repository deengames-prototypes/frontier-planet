extends CanvasLayer

func _process(delta):
	# Overkill but keeps it up-to-date
	# TODO: have an event bus and hook to that on points/level-up
	
	$Label.text = "Community level: " + str(Globals.community_level)
	$Label.text += "\nXP: " + str(Globals.community_xp) + "/" + str(Globals.COMMUNITY_XP_TO_LEVEL_UP)
	