extends Area2D

const VELOCITY:int = 35

export var max_x:int

var _tween:Tween

func _ready():
	_setup_move()

func _setup_move():
	var target_x = randi() % max_x
	var distance = abs(self.position.x - target_x)
	
	while distance < 100:
		target_x = randi() % max_x
		distance = abs(self.position.x - target_x)
		
	var travel_time = 1.0 * distance / VELOCITY 
	var delay = 1 + (randf() * 3)
	
	if _tween != null:
		remove_child(_tween)
		_tween.queue_free()
		
	_tween = Tween.new()
	_tween.interpolate_property(self, "position", self.position, Vector2(target_x, self.position.y), travel_time, Tween.TRANS_LINEAR, Tween.EASE_IN_OUT, delay)
	_tween.connect("tween_completed", self, "_move_again")
	_tween.start()
	add_child(_tween)

func _move_again(a, b):
	self._setup_move()