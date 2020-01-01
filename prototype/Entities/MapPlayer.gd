extends KinematicBody2D

const MOVE_SPEED:int = 200 # px/s
var _velocity:Vector2 = Vector2.ZERO

func _process(delta):
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