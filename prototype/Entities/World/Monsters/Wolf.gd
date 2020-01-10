extends "res://Entities/World/Monsters/HurtPlayerOnTouch.gd"

const HUNTING_RANGE = 500
const VELOCITY = 175
const RECHARGE_MULTIPLIER = 2 # 4 = recharge 5x as fast as exhaustion

var MAX_ENERGY_SECONDS = 5
var _energy_left_seconds = MAX_ENERGY_SECONDS
var _resting_since = 0

func _ready():
	damage_per_second = 40

func _process(delta):
	var player_pos = Globals.player.global_position
	var my_pos = self.global_position
	
	var distance = sqrt(pow(my_pos.x - player_pos.x, 2)
		+ pow(my_pos.y - player_pos.y, 2))
	
	if _resting_since == 0 and distance <= HUNTING_RANGE:
		if _energy_left_seconds > 0:
			
			var direction = my_pos.direction_to(player_pos)
			self.position += direction * VELOCITY * delta
			_energy_left_seconds -= delta
		else:
			_resting_since = OS.get_ticks_msec()
	else:
		if _energy_left_seconds < MAX_ENERGY_SECONDS:
			_energy_left_seconds += delta * RECHARGE_MULTIPLIER
			if _energy_left_seconds >= MAX_ENERGY_SECONDS:
				_energy_left_seconds = MAX_ENERGY_SECONDS
				_resting_since = 0