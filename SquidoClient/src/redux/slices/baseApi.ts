/// <reference types="vite/client" />
const API_URL = import.meta.env.VITE_API_URL;

interface BaseApiOptions {
  method?: "GET" | "POST" | "PUT" | "PATCH" | "DELETE";
  body?: any;
  auth?: boolean; // If true, send Authorization header
}

export async function baseApiRequest<T>(endpoint: string, options: BaseApiOptions = {}): Promise<T> {
  const token = localStorage.getItem("token"); // or sessionStorage if you prefer

  const headers: HeadersInit = {
    "Content-Type": "application/json",
  };

  if (options.auth && token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const response = await fetch(`${API_URL}${endpoint}`, {
    method: options.method || "GET",
    headers,
    body: options.body ? JSON.stringify(options.body) : undefined,
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || "API Request Failed");
  }

  return response.json();
}
