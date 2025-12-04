import axios from "axios";

// הגדרת כתובת API ברירת מחדל
const api = axios.create({
  baseURL: "http://localhost:5000" // עדכני לפי הפורט של ה-API שלך
});

// Interceptor ללכידת שגיאות
api.interceptors.response.use(
  response => response,
  error => {
    console.error("API Error:", error);
    return Promise.reject(error);
  }
);

// פונקציות קריאה ל-API
export const getTasks = () => api.get("/tasks");
export const addTask = (task) => api.post("/tasks", task);
export const updateTask = (id, task) => api.put(`/tasks/${id}`, task);
export const deleteTask = (id) => api.delete(`/tasks/${id}`);

export default api;
