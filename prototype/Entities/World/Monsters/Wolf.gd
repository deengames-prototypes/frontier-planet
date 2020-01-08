extends "res://Entities/World/Monsters/HurtPlayerOnTouch.gd"

const HUNTING_RANGE = 9500
const VELOCITY = 200

var MAX_ENERGY_SECONDS = 5
var _energy_left_seconds = MAX_ENERGY_SECONDS
var _resting_since = 0

func _ready():
	damage_per_second = 40

func _process(delta):
	var distance = sqrt(pow(position.x - Globals.player.position.x, 2)
		+ pow(position.y - Globals.player.position.y, 2))
	
	#var d = (Globals.player.position - self.position).normalized()
	#print(str(d * VELOCITY * delta))
	#self.position += d * VELOCITY * delta
	
	#return
	
	if _resting_since == 0 and distance <= HUNTING_RANGE:
		if _energy_left_seconds > 0:
			var direction = (Globals.player.position - self.position).normalized()
			self.position += direction * VELOCITY * delta
			_energy_left_seconds -= delta
		else:
			_resting_since = OS.get_ticks_msec()
			print("resting start")
	else:
		if _energy_left_seconds < MAX_ENERGY_SECONDS:
			_energy_left_seconds += delta
			if _energy_left_seconds >= MAX_ENERGY_SECONDS:
				_energy_left_seconds = MAX_ENERGY_SECONDS
				_resting_since = 0
				print("done resting")