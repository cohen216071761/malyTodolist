import axios from "axios";

// הגדרת כתובת API ברירת מחדל
const api = axios.create({
    // *** כתובת Render הנכונה ***
    baseURL: "https://malytodolistservice.onrender.com"
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
