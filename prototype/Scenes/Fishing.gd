extends Node2D

const Catfish = preload("res://Entities/Fishing/Catfish.tscn")
const FishTileGame = preload("res://UI/FishTileGame.tscn")
const Jellyfish = preload("res://Entities/Fishing/Jellyfish.tscn")
const SceneManagement = preload("res://Scripts/SceneManagement.gd")
const Sockeye = preload("res://Entities/Fishing/Sockeye.tscn")
const WorldMap = preload("res://Scenes/WorldMap.tscn")

const PROGRESS_BAR_VELOCITY = 96 # 960 = ~full cycle in 1s
const BOBBER_MOVES = 5
const BOBBER_MOVE_AMOUNT = 32

var _horizontal_percent = -1
var _vertical_percent = -1
var _increasing_value = true
var _paused = false
var _bobber_moves_left = BOBBER_MOVES
var _fish_name = ""

onready var HORIZONTAL_RANGE = int($Ocean.margin_right - $RiverBank.margin_right - $Bobber.get_child(0).margin_right)
onready var VERTICAL_RANGE = int($Ocean.margin_bottom - $Ocean.margin_top - $Bobber.get_child(0).margin_bottom)
onready var BOBBER_HOME = $Bobber.position

func _ready():
	var is_catfish = randi() % 100 <= 50
	var num_jellyfish = 1 + (randi() % 3) # 1-3
	var num_fish = 3 + (randi() % 5) # 3-8
	
	while num_jellyfish > 0:
		num_jellyfish -= 1
		var jellyfish = Jellyfish.instance()
		var position = _get_random_ocean_position(jellyfish)
		jellyfish.max_coordinates = Vector2($Ocean.margin_right, $Ocean.margin_bottom)
		jellyfish.position = position
		jellyfish.connect("hooked", self, "_on_fish_hooked", [jellyfish])
		$Ocean.add_child(jellyfish)
	
	if is_catfish:
		var catfish = Catfish.instance()
		catfish.max_x = int(HORIZONTAL_RANGE - catfish.get_child(0).margin_right)
		catfish.position.x = randi() % catfish.max_x
		catfish.position.y = VERTICAL_RANGE - catfish.get_child(0).margin_bottom
		catfish.connect("hooked", self, "_on_fish_hooked", [catfish])
		$Ocean.add_child(catfish)
	
	while num_fish > 0:
		num_fish -= 1
		var sockeye = Sockeye.instance()
		
		sockeye.max_position = Vector2(
			HORIZONTAL_RANGE - sockeye.get_child(0).margin_right,
			VERTICAL_RANGE - sockeye.get_child(0).margin_bottom)
		
		sockeye.position = Vector2(
			randi() % int(sockeye.max_position.x),
			randi() % int(sockeye.max_position.y))
			 
		sockeye.connect("hooked", self, "_on_fish_hooked", [sockeye])
		$Ocean.add_child(sockeye)

func _process(delta):
	if _paused:
		return
		
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
				$ProgressBar/Label.text += "\nVertical: " + str(_vertical_percent) + "%\nMoves left: " + str(BOBBER_MOVES)
				$ProgressBar.value = 0
				$Bobber.position.y = $Ocean.margin_top + (_vertical_percent / 100.0) * VERTICAL_RANGE
				
	else:
		if Input.is_action_just_pressed("ui_accept"):
			_reset_bobber()
		elif _bobber_moves_left > 0:
			var moved = false
			
			if Input.is_action_just_pressed("ui_up"):
				$Bobber.position.y -= BOBBER_MOVE_AMOUNT
				moved = true
			if Input.is_action_just_pressed("ui_left"):
				$Bobber.position.x -= BOBBER_MOVE_AMOUNT
				moved = true
			if Input.is_action_just_pressed("ui_down"):
				$Bobber.position.y += BOBBER_MOVE_AMOUNT
				moved = true
			if Input.is_action_just_pressed("ui_right"):
				$Bobber.position.x += BOBBER_MOVE_AMOUNT
				moved = true
			
			if moved:
				_bobber_moves_left -= 1
				$ProgressBar/Label.text = "Moves left: " + str(_bobber_moves_left)

func _get_random_ocean_position(jellyfish):
	var color_rect = jellyfish.get_child(0)
	var x = randi() % int(HORIZONTAL_RANGE - color_rect.margin_right)
	var y = randi() % int(VERTICAL_RANGE - color_rect.margin_bottom)
	
	return Vector2(x, y)

func _reset_bobber():
	$Bobber.position = BOBBER_HOME
	$ProgressBar/Label.text = ""
	_horizontal_percent = -1
	_vertical_percent = -1
	_bobber_moves_left = BOBBER_MOVES

func _on_fish_hooked(fish):
	if not _paused:
		_paused = true
		_reset_bobber()
		
		self._fish_name = Globals.sanitize_name(fish.name)
		
		var mini_game = FishTileGame.instance()
		mini_game.connect("game_over", self, "_catch_fish_done")
		var color_rect = fish.get_child(0)
		mini_game.set_fish(color_rect)
		add_child(mini_game)
	
func _catch_fish_done(caught_fish):
	_paused = false
	if caught_fish:
		Globals.player_inventory.append(_fish_name)
	
func _unhandled_key_input(event):
	if event.is_pressed() and event.is_action_pressed("ui_cancel"):
		if _horizontal_percent != -1 or _vertical_percent != -1:
			_reset_bobber()
		else:
			# escape with no fishing = bye BYE
			var map = WorldMap.instance()
			# null = restore old position
			call_deferred("_change_map", map)

func _change_map(map):
	SceneManagement.change_map_to(get_tree(), map, null)