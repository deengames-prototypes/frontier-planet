extends StaticBody2D

export var destination:String setget set_destination, get_destination
export var location:String setget set_location, get_location

func set_destination(value):
	$Door.destination = value

func get_destination():
	return $Door.destination
	
func set_location(value):
	$Door.location = value

func get_location():
	return $Door.location