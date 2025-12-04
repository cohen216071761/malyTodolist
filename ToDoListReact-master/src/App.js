import React, { useState, useEffect } from "react";
import axios from "axios";

// אינסטנס של axios עם baseURL מה־.env
const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL,
});

function App() {
  const [tasks, setTasks] = useState([]);
  const [newTask, setNewTask] = useState("");

  // טוען את כל המטלות
  const loadTasks = async () => {
    console.log("Loading tasks...");
    try {
      const res = await api.get("/tasks");
      console.log("API response:", res.data);
      
      // המרה אחידה - תומכת גם באותיות גדולות וגם קטנות
      const tasksArray = Array.isArray(res.data) 
        ? res.data.map(t => ({
            id: t.id || t.Id,
            name: t.name || t.Name,
            isComplete: t.isComplete ?? t.IsComplete ?? false
          })) 
        : [];
      
      console.log("Processed tasks:", tasksArray);
      setTasks(tasksArray);
    } catch (err) {
      console.error("Error fetching tasks:", err);
    }
  };

  useEffect(() => {
    loadTasks();
  }, []);

  // מוסיף מטלה חדשה
  const addTask = async () => {
    if (!newTask.trim()) return;

    try {
      const res = await api.post("/tasks", { Name: newTask, IsComplete: false });
      
      const newTaskFromServer = {
        id: res.data.id || res.data.Id,
        name: res.data.name || res.data.Name,
        isComplete: res.data.isComplete ?? res.data.IsComplete ?? false
      };

      setTasks([...tasks, newTaskFromServer]);
      setNewTask("");
    } catch (err) {
      console.error("Error adding task:", err);
    }
  };

  // מסמן מטלה כהושלמה / לא
  const toggleComplete = async (task) => {
    try {
      await api.put(`/tasks/${task.id}`, { 
        Name: task.name, 
        IsComplete: !task.isComplete 
      });
      
      setTasks(tasks.map(t => 
        t.id === task.id ? { ...t, isComplete: !task.isComplete } : t
      ));
    } catch (err) {
      console.error("Error updating task:", err);
    }
  };

  // מוחק מטלה
  const deleteTask = async (task) => {
    try {
      await api.delete(`/tasks/${task.id}`);
      setTasks(tasks.filter(t => t.id !== task.id));
    } catch (err) {
      console.error("Error deleting task:", err);
    }
  };

  return (
    <div style={{ maxWidth: "500px", margin: "50px auto", fontFamily: "Arial" }}>
      <h1>Todo List</h1>

      <div style={{ display: "flex", marginBottom: "20px" }}>
        <input
          type="text"
          value={newTask}
          onChange={e => setNewTask(e.target.value)}
          placeholder="הכנס מטלה חדשה"
          style={{ flex: 1, padding: "8px" }}
        />
        <button onClick={addTask} style={{ padding: "8px 16px", marginLeft: "10px" }}>
          הוסף
        </button>
      </div>

      {tasks.length === 0 ? (
        <p>אין מטלות להצגה</p>
      ) : (
        <ul style={{ listStyle: "none", padding: 0 }}>
          {tasks.map(task => (
            <li key={task.id} style={{ display: "flex", alignItems: "center", marginBottom: "10px" }}>
              <input
                type="checkbox"
                checked={task.isComplete}
                onChange={() => toggleComplete(task)}
                style={{ marginRight: "10px" }}
              />
              <span style={{ 
                flex: 1, 
                textDecoration: task.isComplete ? "line-through" : "none" 
              }}>
                {task.name}
              </span>
              <button onClick={() => deleteTask(task)} style={{ padding: "4px 8px" }}>
                מחק
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

export default App;