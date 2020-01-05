extends KinematicBody2D

const Inventory = preload("res://UI/Inventory.tscn")

const MOVE_SPEED:int = 200 # px/s
var _velocity:Vector2 = Vector2.ZERO
var _frozen = false

# Inventory is here because it doesn't go in any one map
var _inventory_canvas = null

func _ready():
	Globals.player = self

func freeze():
	_frozen = true
	
func unfreeze():
	_frozen = false
	
func _process(delta):
	if not _frozen:
		_velocity = Vector2.ZERO
		
		if Input.is_action_pressed("ui_up"):
			_velocity.y = -MOVE_SPEED
		elif Input.is_action_pressed("ui_down"):
			_velocity.y = MOVE_SPEED
		
		if Input.is_action_pressed("ui_left"):
			_velocity.x = -MOVE_SPEED
		elif Input.is_action_pressed("ui_right"):
			_velocity.x = MOVE_SPEED
	
		self.move_and_slide(_velocity)
	
		if Input.is_action_just_pressed("toggle_inventory"):
			if _inventory_canvas == null:
				_inventory_canvas = CanvasLayer.new()
				var instance = Inventory.instance()
				_inventory_canvas.add_child(instance)
				instance.position = Vector2(10, 10)
				get_parent().add_child(_inventory_canvas)
			else:
				get_parent().remove_child(_inventory_canvas)
				_inventory_canvas.free()
				_inventory_canvas = null