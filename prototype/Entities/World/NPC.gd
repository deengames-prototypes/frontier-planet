extends Area2D

const DialogPanel = preload("res://UI/DialogPanel.tscn")

export var quest_dialogs:Array = []
export var quest_requires:Array = []
export var quest_completions:Array = []
var _panel#:DialogPanel
var current_quest_number = 0

func _on_Door_body_entered(body):
	var dialog = quest_dialogs[current_quest_number]
	if body.name == "MapPlayer":
		_panel = DialogPanel.instance()
		_panel.set_text(dialog)
		_panel.position.y += 100
		add_child(_panel)

func _on_NPC_body_exited(body):
	if body.name == "MapPlayer":
		remove_child(_panel)
		_panel.free()
