extends Area2D

const DialogPanel = preload("res://UI/DialogPanel.tscn")

export var quest_dialogs:Array = []
export var quest_requires:Array = []
export var quest_completions:Array = []
export var dialog:String = ""
export var npc_name:String = ""

var _panel#:DialogPanel
var current_quest_number = 0

func _ready():
	self.current_quest_number = Globals.quest_indicies[self.npc_name]

func _on_Door_body_entered(body):
	if body.name == "MapPlayer":
		_panel = DialogPanel.instance()
		var dialog = _figure_out_dialog()
		_panel.set_text(dialog)
		_panel.position.y += 100
		add_child(_panel)

func _on_NPC_body_exited(body):
	if body.name == "MapPlayer":
		remove_child(_panel)
		_panel.free()

func _figure_out_dialog():
	if dialog != "":
		return dialog
	else:
		var quest_index = current_quest_number
		if quest_index < len(quest_dialogs):
			var need = quest_requires[quest_index]
			var inventory_index = Globals.player_inventory.find(need)
			
			if inventory_index > -1:
				current_quest_number += 1
				Globals.quest_indicies[self.npc_name] = current_quest_number
				Globals.player_inventory.remove(inventory_index)
				var message = quest_completions[quest_index]
				if quest_index + 1 < len(quest_dialogs):
					message += "\n" + quest_dialogs[quest_index + 1]
				return message
			else:
				return quest_dialogs[quest_index]
		else:
			# Done all quests, give final victory message again
			return quest_completions[quest_index - 1]