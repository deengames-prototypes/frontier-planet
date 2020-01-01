extends Node2D

const Jellyfish = preload("res://Entities/Fishing/Jellyfish.tscn")

const PROGRESS_BAR_VELOCITY = 96 # 960 = ~full cycle in 1s

var _horizontal_percent = -1
var _vertical_percent = -1
var _increasing_value = true

onready var HORIZONTAL_RANGE = int($Ocean.margin_right - $RiverBank.margin_right - $Bobber.get_child(0).margin_right)
onready var VERTICAL_RANGE = int($Ocean.margin_bottom - $Ocean.margin_top - $Bobber.get_child(0).margin_bottom)
onready var BOBBER_HOME = $Bobber.position

func _ready():
	var is_catfish = randi() % 100 <= 50
	var num_jellyfish = 1 + (randi() % 3) # 1-3
	var num_fish = 3 + (randi() % 5) # 3-8
	
	while num_jellyfish > 0:
		num_jellyfish -= 1
		var position = _get_random_ocean_position()
		var jellyfish = Jellyfish.instance()
		jellyfish.max_coordinates = Vector2($Ocean.margin_right, $Ocean.margin_bottom)
		jellyfish.position = position
		$Ocean.add_child(jellyfish)

func _process(delta):
	if _horizontal_percent == -1 or _vertical_percent == -1:
		if _increasing_value:
			$ProgressBar.value += (PROGRESS_BAR_VELOCITY * delta)
			if $ProgressBar.value >= $ProgressBar.max_value:
				_increasing_value = false
		else:
			$ProgressBar.value -= (PROGRESS_BAR_VELOCITY * delta)
			if $ProgressBar.value <= 0:
				_increasing_value = true
		
		if Input.is_action_just_pressed("ui_accept"):
			if _horizontal_percent == -1:
				_horizontal_percent = $ProgressBar.value
				$ProgressBar/Label.text = "Horizontal: " + str(_horizontal_percent) + "%"
				$ProgressBar.value = 0
				$Bobber.position.x = $RiverBank.margin_right + (_horizontal_percent / 100.0) * HORIZONTAL_RANGE
			else:
				_vertical_percent = $ProgressBar.value
				$ProgressBar/Label.text += "\nVertical: " + str(_vertical_percent) + "%"
				$ProgressBar.value = 0
				$Bobber.position.y = $Ocean.margin_top + (_vertical_percent / 100.0) * VERTICAL_RANGE
	elif Input.is_action_just_pressed("ui_accept"):
		$Bobber.position = BOBBER_HOME
		$ProgressBar/Label.text = ""
		_horizontal_percent = -1
		_vertical_percent = -1

func _get_random_ocean_position():
	var x = randi() % HORIZONTAL_RANGE
	var y = randi() % VERTICAL_RANGE
	
	return Vector2(x, y)