extends Node

const MapPlayer = preload("res://Entities/MapPlayer.tscn")

static func change_scene_to(tree, scene_instance):
	var root = tree.get_root()
	var current_scene = get_current_scene(root)
	root.remove_child(current_scene)
	current_scene.queue_free()
		
	current_scene = scene_instance	
	# http://docs.godotengine.org/en/3.0/getting_started/step_by_step/singletons_autoload.html?highlight=change_scene	
	root.add_child(current_scene)
	# Optional, to make it compatible with the SceneTree.change_scene() API.
	tree.set_current_scene(current_scene)
	
	####### adds player
	var player = MapPlayer.instance()
	player.position = Vector2(100, 100)
	scene_instance.add_child(player)

static func get_current_scene(root):
	var child_count = root.get_child_count()
	var last_child = root.get_child(0)
	
	for i in range(child_count):
		var child = root.get_child(i)
		last_child = child
	
	return last_child