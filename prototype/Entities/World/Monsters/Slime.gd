extends "res://Entities/World/Monsters/HurtPlayerOnTouch.gd"

const CHANGE_MIND_AFTER_SECONDS = 1
const FOREST_SIZE = Vector2(3000, 2500)
const VELOCITY = 70

var _destination = Vector2.ZERO
var _set_dest_on = 0

func _ready():
	damage_per_second = 30
	_set_destination()

func _process(delta):
	var elapsed = (OS.get_ticks_msec() - _set_dest_on) / 1000
	
	if elapsed >= CHANGE_MIND_AFTER_SECONDS:
		_set_destination()
	
	var direction = (_destination - position).normalized()
	self.position += direction * VELOCITY * delta

func _set_destination():
	_destination = Vector2(
		randi() % int(FOREST_SIZE.x),
		randi() % int(FOREST_SIZE.y))
		
	_set_dest_on = OS.get_ticks_msec()
