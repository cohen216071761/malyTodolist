// // import axios from "axios";

// // // כתובת ה-API שלך
// // const apiUrl = "http://localhost:5071";

// // // יצירת instance של axios עם config בסיסי
// // const api = axios.create({
// //   baseURL: apiUrl,
// //   headers: {
// //     "Content-Type": "application/json",
// //   },
// // });

// // // Interceptor לטיפול בשגיאות
// // api.interceptors.response.use(
// //   (response) => response,
// //   (error) => {
// //     if (error.response && error.response.status === 401) {
// //       console.warn("Unauthorized - redirect to login");
// //       window.location.href = "/login"; // או כל דף לוגין שהגדרת
// //     }
// //     return Promise.reject(error);
// //   }
// // );

// // export default {
// //   // שליפת כל המשימות
// //   getTasks: async () => {
// //     try {
// //       const result = await api.get("/tasks"); // GET http://localhost:5000/tasks
// //       return result.data;
// //     } catch (err) {
// //       console.error("Error fetching tasks:", err);
// //       return [];
// //     }
// //   },

// //   // הוספת משימה חדשה
// //   addTask: async (name) => {
// //     try {
// //       const result = await api.post("/tasks", { name, isComplete: false });
// //       return result.data; // מחזיר את האובייקט שנוסף
// //     } catch (err) {
// //       console.error("Error adding task:", err);
// //       return null;
// //     }
// //   },

// //   // שינוי סטטוס של משימה (סימון כבוצע או לא)
// //   setCompleted: async (id, isComplete) => {
// //     try {
// //       const result = await api.put(`/tasks/${id}`, { isComplete });
// //       return result.data;
// //     } catch (err) {
// //       console.error("Error updating task:", err);
// //       return null;
// //     }
// //   },

// //   // מחיקת משימה לפי ID
// //   deleteTask: async (id) => {
// //     try {
// //       const result = await api.delete(`/tasks/${id}`);
// //       return result.data;
// //     } catch (err) {
// //       console.error("Error deleting task:", err);
// //       return null;
// //     }
// //   },
// // };

// import axios from "axios";

// const api = axios.create({
//   baseURL: "http://localhost:5071",
// });

// export const service = {
//   getTasks: async () => {
//     try {
//       const response = await api.get("/tasks");
//       return response.data;
//     } catch (err) {
//       console.error("Error fetching tasks:", err);
//       return [];
//     }
//   },

//   setCompleted: async (id, isComplete) => {
//     if (!id) {
//       console.error("Task ID is undefined");
//       return null;
//     }
//     try {
//       const result = await api.put(`/tasks/${id}`, { isComplete });
//       return result.data;
//     } catch (err) {
//       console.error("Error updating task:", err);
//       return null;
//     }
//   },
// };

// export default service;

import axios from "axios";

// הגדרת כתובת ה־API שלך
const api = axios.create({
  baseURL: "http://localhost:5071", // ודאי ש־API שלך רץ פה
});

const service = {
  // פונקציה לקבלת כל המטלות
  getTasks: async () => {
    try {
      const result = await api.get("/tasks");
      return result.data;
    } catch (err) {
      console.error("Error fetching tasks:", err);
      return [];
    }
  },

  // פונקציה לעדכון סטטוס השלמה של מטלה
  setCompleted: async (id, isComplete) => {
    try {
      const result = await api.put(`/tasks/${id}`, { isComplete });
      return result.data;
    } catch (err) {
      console.error("Error updating task:", err);
      return null;
    }
  },

  // אפשר להוסיף כאן פונקציות נוספות כמו createTask, deleteTask וכו'
};

export default service;
