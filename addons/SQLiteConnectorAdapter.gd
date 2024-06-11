extends Node

var database : SQLite

const SQLITE_OK = 0
const SQLITE_ROW = 100
const SQLITE_DONE = 101

# Called when the node enters the scene tree for the first time.
func _ready():
	_open_database("res://data.db")
	_createDatabaseTables()
	
	
func _exit_tree():
	database.close_db()

func _open_database(db_path: String) -> bool:

	database = SQLite.new()
	database.path = db_path
	if database.open_db() != true:
		print("Failed to open database")
		return false
	return true

func _createDatabaseTables():
	var create_shopping_list_table = """
		CREATE TABLE IF NOT EXISTS shoppingList (
			id INTEGER PRIMARY KEY AUTOINCREMENT,
			name TEXT,
			owner TEXT
		);
	"""
	var create_items_table = """
		CREATE TABLE IF NOT EXISTS items (
			id INTEGER PRIMARY KEY AUTOINCREMENT,
			shoppingList INTEGER,
			name TEXT,
			quantity TEXT,
			FOREIGN KEY(shoppingList) REFERENCES shoppingList(id)
		);
	"""
	var create_shared_with_table = """
		CREATE TABLE IF NOT EXISTS sharedWith (
			id INTEGER PRIMARY KEY AUTOINCREMENT,
			shoppingList INTEGER,
			text TEXT,
			FOREIGN KEY(shoppingList) REFERENCES shoppingList(id)
		);
	"""
	
	database.query(create_shopping_list_table)
	database.query(create_items_table)
	database.query(create_shared_with_table)
	print("Tables created successfully")

func _send_new_shoppinglist_request(shared_with, name, owner):
	if not _open_database("res://data.db"):
		return "Failed to open database"

	database.query("BEGIN;") # Start a transaction

	var insert_shopping_list = "INSERT INTO shoppingList (name, owner) VALUES (?, ?);"
	var result = database.query_with_bindings(insert_shopping_list, [name, owner])
	
	if not result:
		database.query("ROLLBACK;") # Rollback transaction if there's an error
		
		return "Failed to insert shopping list"

	var shopping_list_id = database.last_insert_rowid
	
	for user in shared_with:
		var insert_shared_with = "INSERT INTO sharedWith (shoppingList, text) VALUES (?, ?);"
		result = database.query_with_bindings(insert_shared_with, [shopping_list_id, user])
		
		if not result:
			database.query("ROLLBACK;") # Rollback transaction if there's an error
			
			return "Failed to insert shared with"

	database.query("COMMIT;") # Commit the transaction
	
	return "OK"

func _get_shoppinglists_owner_request(email):
	if not _open_database("res://data.db"):
		return []

	var query = "SELECT * FROM shoppingList WHERE owner = ?;"
	var result = database.query_with_bindings(query, [email])
	
	if not result:
		return []

	var lists = database.query_result
	return lists

func _get_shoppinglists_shared_request(email):
	if not _open_database("res://data.db"):
		return "Failed to open database"

	var query = """
		SELECT shoppingList.* FROM shoppingList
		INNER JOIN sharedWith ON shoppingList.id = sharedWith.shoppingList
		WHERE sharedWith.text = ?;
	"""
	var result = database.query_with_bindings(query, [email])
	
	if result != true:
		return result

	var lists = database.query_result
	
	return lists

#func _get_shoppinglist_request(shopping_list_id):
#	if not _open_database("res://data.db"):
#		return "Failed to open database"
#
#	var query = "SELECT * FROM shoppingList WHERE id = ?;"
#	var result = database.query_with_bindings(query, [shopping_list_id])
##	
#	if result != true:
#		return result
#
##	var list = null
#	if len(database.query_result) > 0:
#		list = database.query_result[0]
#	
#	return list

func _get_shoppinglist_request(shopping_list_id: int):
	if not _open_database("res://data.db"):
		return "Failed to open database"

	var query = "SELECT * FROM shoppingList WHERE id = ?;"
	var result = database.query_with_bindings(query, [shopping_list_id])

	if result != true:
		return result

	var list = {}
	if len(database.query_result) > 0:
		list = database.query_result[0]

	var items_query = "SELECT * FROM items WHERE shoppingList = ?;"
	var items_result = database.query_with_bindings(items_query, [shopping_list_id])

	if items_result != true:
		return items_result
		
	list["items"] = database.query_result
	
	return list

func _delete_shoppinglist_request(shopping_list_id):
	if not _open_database("res://data.db"):
		return "Failed to open database"

	# Start a transaction
	database.query("BEGIN;")

	# Delete items associated with the shopping list
	var delete_items_query = "DELETE FROM items WHERE shoppingList = ?;"
	var result = database.query_with_bindings(delete_items_query, [shopping_list_id])

	if result != true:
		database.query("ROLLBACK;")
		return result

	var delete_sharedWith_query = "DELETE FROM sharedWith WHERE shoppingList = ?;"
	result = database.query_with_bindings(delete_sharedWith_query, [shopping_list_id])

	if result != true:
		database.query("ROLLBACK;")
		return result

	# Delete the shopping list itself
	var delete_shopping_list_query = "DELETE FROM shoppingList WHERE id = ?;"
	result = database.query_with_bindings(delete_shopping_list_query, [shopping_list_id])

	if result != true:
		database.query("ROLLBACK;")
		return result
		

	# Commit the transaction
	database.query("COMMIT;")
	return "Shopping list and associated items deleted successfully" 

func _add_item(shopping_list_id, name, quantity):
	print("add item")
	if not _open_database("res://data.db"):
		return false
	
	var query = "INSERT INTO items (shoppingList, name, quantity) VALUES (?, ?, ?);"
	var result = database.query_with_bindings(query, [shopping_list_id, name, quantity])
	print("Added item: "+shopping_list_id + " "+ name)
	if result != true:
		return false
	
	return true
	
func _delete_items(shopping_list_id):
	if not _open_database("res://data.db"):
		return false
	
	var query = "DELETE FROM items WHERE shoppingList = ?;"
	var result = database.query_with_bindings(query, [shopping_list_id])
	
	if result != true:
		return false
	
	return true

func _send_auth_request(function, email, password):
	# This function would typically interact with an external authentication service.
	# Implementing a mock function for now.
	return {
		"email": email,
		"token": "mock_token"
	}
