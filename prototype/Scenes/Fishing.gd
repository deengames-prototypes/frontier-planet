extends Node2D

const PROGRESS_BAR_VELOCITY = 96 # 960 = ~full cycle in 1s

var _horizontal_percent = -1
var _vertical_percent = -1

var _casting_horizontally = true
var _increasing_value = true

onready var HORIZONTAL_RANGE = $Ocean.margin_right - $RiverBank.margin_right - $Bobber.get_child(0).margin_right
onready var VERTICAL_RANGE = $Ocean.margin_bottom - $Ocean.margin_top - $Bobber.get_child(0).margin_bottom

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
			if _casting_horizontally:
				_horizontal_percent = $ProgressBar.value
				_casting_horizontally = false
				$ProgressBar/Label.text = "Horizontal: " + str(_horizontal_percent) + "%"
				$ProgressBar.value = 0
				$Bobber.position.x = $RiverBank.margin_right + (_horizontal_percent / 100.0) * HORIZONTAL_RANGE
			else:
				_vertical_percent = $ProgressBar.value
				$ProgressBar/Label.text += "\nVertical: " + str(_vertical_percent) + "%"
				$ProgressBar.value = 0
				$Bobber.position.y = $Ocean.margin_top + (_vertical_percent / 100.0) * VERTICAL_RANGE
				