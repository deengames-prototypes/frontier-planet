extends Panel

export var sequence_length = 50
export var show_tile_delay_seconds = 0.5

var _correct_sequence = []

func _ready():
	yield(get_tree().create_timer(5), 'timeout')
	randomize()
	self.sequence_length = sequence_length
	while len(_correct_sequence) < sequence_length:
		var index = randi() % $Node2D.get_child_count()
		var next = $Node2D.get_child(index)
		_correct_sequence.append(next.pitch_multiplier)
	
	_dim_tiles(0.5)
	_play_sequence()

func _dim_tiles(target_alpha):
	for child in $Node2D.get_children():
		child.modulate.a = target_alpha

func _play_sequence():
	for multiplier in _correct_sequence:
		var tile = _get_tile(multiplier)
		tile.modulate.a = 1
		
		$AudioStreamPlayer.pitch_scale = multiplier
		$AudioStreamPlayer.play()
		yield(get_tree().create_timer(show_tile_delay_seconds), 'timeout')
		
		tile.modulate.a = 0.5
		yield(get_tree().create_timer(0.1), 'timeout')
	
	_dim_tiles(1)

func _get_tile(multiplier):
	for tile in $Node2D.get_children():
		if tile.pitch_multiplier == multiplier:
			return tile
	
	return null