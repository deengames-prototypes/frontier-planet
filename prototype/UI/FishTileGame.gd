extends Node2D

var num_arrows = 5
signal game_over

var _fish_moves = []
var _fish_width:float
var _fish_height:float
var _ready = false

var _answers = []

func _ready():
	for arrow in $Panel/Arrows.get_children():
		arrow.visible = false
	
	for arrow in $Panel/Answers.get_children():
		arrow.visible = false
		
	$Fish.position = Vector2($Panel.margin_right / 2, $Panel.margin_bottom / 2)
	var directions = ["up", "right", "down", "left"]

	yield(get_tree().create_timer(1), "timeout")
	
	while len(_fish_moves) < num_arrows:
		var direction = directions[randi() % len(directions)]
		_fish_moves.append(direction)
	
	# Move dem fish moves
	var done = 0
	while done < len(_fish_moves):
		var next_move = _fish_moves[done]
		var translation = _get_translation(next_move)
		var rotation = _get_rotation(next_move)
		
		var arrow = $Panel/Arrows.get_child(done)
		arrow.rotation_degrees = rotation
		arrow.visible = true
		
		$Fish.position += translation
		yield(get_tree().create_timer(1), "timeout")
		done += 1

	for arrow in $Panel/Arrows.get_children():
		arrow.visible = false
		
	_ready = true

func _unhandled_key_input(event):
	if _ready and len(_answers) < num_arrows and event.is_pressed():
		var arrow:Sprite = $Panel/Answers.get_child(len(_answers))
		$Panel/Arrows.get_child(len(_answers)).visible = true
		var answer:int = event.scancode
		if event.scancode == KEY_UP:
			arrow.rotation_degrees = 0
			_answers.append("up")
		elif event.scancode == KEY_RIGHT:
			arrow.rotation_degrees = 90
			_answers.append("right")
		elif event.scancode == KEY_DOWN:
			arrow.rotation_degrees = 180
			_answers.append("down")
		elif event.scancode == KEY_LEFT:
			arrow.rotation_degrees = 270
			_answers.append("left")
		arrow.visible = true
		
		if len(_answers) == num_arrows:
			var success = true
			for i in range(num_arrows):
				if _answers[i] != _fish_moves[i]:
					success = false
					break
			
			if success:
				$Label.text = "Caught it!"
			else:
				$Label.text = "It got away!"
			$Label.visible = true
			yield(get_tree().create_timer(2), "timeout")
			
			emit_signal("game_over", success)
			self.get_parent().remove_child(self)
			self.queue_free()

# called BEFORE _ready
func set_fish(target:ColorRect):
	var fish_color = $Fish.get_child(0)
	fish_color.color = target.color
	fish_color.margin_bottom = target.margin_bottom
	fish_color.margin_right = target.margin_right
	
	_fish_width = target.margin_right
	_fish_height = target.margin_bottom
	$Fish.position.x -= _fish_width / 2
	$Fish.position.y -= _fish_height / 2

func _get_rotation(move):
	if move == "up": return 0
	elif move == "right": return 90
	elif move == "down": return 180
	elif move == "left": return 270

func _get_translation(move):
	if move == "up": return Vector2(0, -_fish_height)
	elif move == "down": return Vector2(0, _fish_height)
	elif move == "right": return Vector2(_fish_width, 0)
	elif move == "left": return Vector2(-_fish_width, 0)