﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using LocalNote.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace LocalNote.Repositories {
    public static class DatabaseRepo {
        private static readonly string conn = "Filename=notesDb.db";

        /// Initializes the database.
        public static async void InitializeDB() {
            using (var db = new SqliteConnection(conn)) {
                // Open the database
                db.Open();

                // Create the command to create a table
                var create = new SqliteCommand {
                    Connection = db,
                    CommandText =
                    @"
                        CREATE TABLE IF NOT EXISTS NotesTable (
                            notes_id INTEGER PRIMARY KEY AUTOINCREMENT, 
                            title nvarchar(100) NOT NULL UNIQUE,
                            content nvarchar(255) NOT NULL
                        );
                    "
                };

                // Execute the command
                try {
                    await create.ExecuteReaderAsync();
                } catch (SqliteException e) {
                    Debug.WriteLine(e);
                }
            }
        }

        /// Drops the table that was initialized. Used for testing.
        public static async void DropDB() {
            using (var db = new SqliteConnection(conn)) {
                // Open the database
                db.Open();

                // Create the command to create a table
                var drop = new SqliteCommand {
                    Connection = db,
                    CommandText =
                    @"
                        DROP TABLE IF EXISTS NotesTable;
                    "
                };

                // Execute the command
                try {
                    await drop.ExecuteReaderAsync();
                } catch (SqliteException e) {
                    Debug.WriteLine(e);
                }
            }
        }

        /// Adds a note to the database.
        /// <param name="note">The note to be added.</param>
        public static async void AddNote(NoteModel note) {
            using (var db = new SqliteConnection(conn)) {
                // Open the database
                db.Open();

                // Create the command to insert notes in the notes table
                var insert = new SqliteCommand {
                    Connection = db,
                    CommandText =
                    @"
                        INSERT INTO NotesTable (title, content)
                        VALUES (@title, @content);
                    "
                };

                // Add the parameters to the command
                try {
                    insert.Parameters.AddWithValue("@title", note.Title);
                    insert.Parameters.AddWithValue("@content", note.Content.Rtf);
                } catch (NullReferenceException e) {
                    Debug.WriteLine("Error occurred: Note is null. Error: " + e);
                    throw new NullReferenceException();
                }

                // Execute the command
                try {
                    await insert.ExecuteReaderAsync();
                } catch (SqliteException e) {
                    Debug.WriteLine("Sqlite exception: " + e);
                }
            }
        }

        /// Updates a note record in the database.
        /// <param name="note">The note to be updated.</param>
        public static async void UpdateNote(NoteModel note) {
            using (var db = new SqliteConnection(conn)) {
                // Open the database
                db.Open();

                // Create the command to delete a note
                var update = new SqliteCommand {
                    Connection = db,
                    CommandText =
                    @"
                        UPDATE NotesTable
                        SET content = @content
                        WHERE title = @title;
                    "
                };

                // Add the parameters to the command
                try {
                    update.Parameters.AddWithValue("@title", note.Title);
                    update.Parameters.AddWithValue("@content", note.Content.Rtf);
                } catch (NullReferenceException e) {
                    Debug.WriteLine("Error occurred: Note is null. Error: " + e);
                    throw new NullReferenceException();
                }

                // Execute the command
                try {
                    await update.ExecuteReaderAsync();
                } catch (SqliteException e) {
                    Debug.WriteLine(e);
                }
            }
        }

        /// Deletes a note record in the database.
        /// <param name="note">The note to be deleted.</param>
        public static async void DeleteNote(NoteModel note) {
            using (var db = new SqliteConnection(conn)) {
                // Open the database
                db.Open();

                // Create the command to delete a note
                var delete = new SqliteCommand {
                    Connection = db,
                    CommandText =
                    @"
                        DELETE FROM NotesTable
                        WHERE title = @title;
                    "
                };

                // Add the parameters to the command
                try {
                    delete.Parameters.AddWithValue("@title", note.Title);
                } catch (NullReferenceException e) {
                    Debug.WriteLine("Error occurred: Note is null. Error: " + e);
                    throw new NullReferenceException();
                }

                // Execute the command
                try {
                    await delete.ExecuteReaderAsync();
                } catch (SqliteException e) {
                    Debug.WriteLine(e);
                }
            }
        }


        /// Gets the specific note from the database based on the title given.
        /// <param name="title">The title of the note.</param>
        /// <returns>The note model of the specific note.</returns>
        public async static Task<NoteModel> GetNote(string title) {
            NoteModel note = null;

            using (var db = new SqliteConnection(conn)) {
                // Open the database
                db.Open();

                // Create the select command to get the specific note in the table
                var select = new SqliteCommand {
                    Connection = db,
                    CommandText = 
                        @"
                            SELECT title, content FROM NotesTable
                            WHERE title = @title;
                        "
                };

                // Add the parameter to the select statement
                try {
                    select.Parameters.AddWithValue("@title", title.Trim());
                } catch (NullReferenceException e) {
                    Debug.WriteLine("Error occurred: Title is null. Error: " + e);
                    throw new NullReferenceException();
                }

                // Execute the command
                SqliteDataReader query = null;
                try {
                    query = await select.ExecuteReaderAsync();
                } catch (SqliteException e) {
                    Debug.WriteLine(e);
                }

                // Set the record returned to the note object
                while (query.Read()) {
                    note = new NoteModel(query.GetString(0), new ContentModel(query.GetString(1), ""));
                }
            }
            return note;
        }

        /// Gets all the notes from the database.
        /// <returns>The notes collection</returns>
        public async static Task<ObservableCollection<NoteModel>> GetNotes() {
            ObservableCollection<NoteModel> notes = new ObservableCollection<NoteModel>();

            using (var db = new SqliteConnection(conn)) {
                // Open the database
                db.Open();

                // Create the select command to get all the notes in the table
                var select = new SqliteCommand {
                    Connection = db,
                    CommandText = "SELECT title, content FROM NotesTable ORDER BY title;"
                };

                // Execute the command
                SqliteDataReader query = null;
                try {
                    query = await select.ExecuteReaderAsync();
                } catch (SqliteException e) {
                    Debug.WriteLine(e);
                }

                // Add all the records returned to the notes collection
                while (query.Read()) {
                    notes.Add(new NoteModel(query.GetString(0), new ContentModel(query.GetString(1), "")));
                }
            }
            // Return the notes collection
            return notes;
        }

        /// Checks if the note already exist in the database.
        /// <param name="note">The note to be checked.</param>
        /// <returns>True if it does, otherwise returns false.</returns>
        public async static Task<bool> NoteExist(NoteModel note) {
            using (var db = new SqliteConnection(conn)) {
                // Open the database
                db.Open();

                // Create the select command to get all the notes in the table
                var select = new SqliteCommand {
                    Connection = db,
                    CommandText = 
                    @"
                        SELECT * FROM NotesTable 
                        WHERE title LIKE @title;
                    "
                };

                // Add the parameters to the command
                try {
                    select.Parameters.AddWithValue("@title", note.Title);
                } catch (NullReferenceException e) { 
                    Debug.WriteLine("Error occurred: Note is null. Error: " + e);
                    throw new NullReferenceException();
                }

                // Execute the command
                SqliteDataReader query = null;
                try {
                    query = await select.ExecuteReaderAsync();
                } catch (SqliteException e) {
                    Debug.WriteLine(e);
                }

                // If we can read the query, then the note already exists
                while (query.Read()) {
                    return true;
                }
                // Otherwise, it doesn't exist
                return false;
            }
        }
    }
}
