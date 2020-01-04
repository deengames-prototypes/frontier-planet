extends Node

const MapPlayer = preload("res://Entities/World/MapPlayer.tscn")

var _current_scene

static func change_map_to(tree, scene_instance, node_name):
	var current_scene = change_scene_to(tree, scene_instance)
	
	####### adds player
	var player = MapPlayer.instance()
	current_scene.add_child(player)
	if current_scene.has_method("move_player_to"):
		current_scene.move_player_to(node_name)
	else:
		player.position = Vector2(100, 100)

static func change_scene_to(tree, scene_instance):
	var root = tree.get_root()
	var current_scene = root.get_child(root.get_child_count() - 1)
	root.remove_child(current_scene)
	current_scene.queue_free()
	
	current_scene = scene_instance
	# http://docs.godotengine.org/en/3.0/getting_started/step_by_step/singletons_autoload.html?highlight=change_scene	
	root.add_child(current_scene)
	# Optional, to make it compatible with the SceneTree.change_scene() API.
	tree.set_current_scene(current_scene)
	
	return current_scene