tool
extends Node2D

export var width:int = 400 setget set_width, get_width
export var height:int = 200 setget set_height, get_height

func set_text(text):
	$Label.text = text

func set_width(value):
	if has_child("Panel") and $Panel != null:
		$Panel.margin_right = value
		$Label.margin_right = value - 10

func get_width():
	return $Panel.margin_right

func set_height(value):
	if has_child("Panel") and $Panel != null:
		$Panel.margin_bottom = value
		$Label.margin_bottom = value - 10

func get_height():
	return $Panel.margin_bottom

func has_child(child_name):
	for child in self.get_children():
		if child.name == child_name:
			return true
	return false