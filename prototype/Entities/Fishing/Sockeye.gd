extends Area2D

const VELOCITY:int = 50

export var max_position:Vector2

var _tween:Tween

func _ready():
	_setup_move()

func _setup_move():
	var target = Vector2(
		randi() % int(max_position.x),
		randi() % int(max_position.y))
	
	var distance = sqrt(pow(self.position.x - target.x, 2) + pow(self.position.y - target.y, 2))
	var travel_time = 1.0 * distance / VELOCITY 
	var delay = 1 + (randf() * 3)
	
	if _tween != null:
		remove_child(_tween)
		_tween.queue_free()
		
	_tween = Tween.new()
	_tween.interpolate_property(self, "position", self.position, target, travel_time, Tween.TRANS_LINEAR, Tween.EASE_IN_OUT, delay)
	_tween.connect("tween_completed", self, "_move_again")
	_tween.start()
	add_child(_tween)

func _move_again(a, b):
	self._setup_move()