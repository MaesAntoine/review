import { initializeApp } from 'firebase/app';
import { getAuth, signInAnonymously, onAuthStateChanged, type User } from 'firebase/auth';
import { writable } from 'svelte/store';

// Your web app's Firebase configuration
const firebaseConfig = {
  apiKey: "AIzaSyBqt8U4F2GTOqCDGF1Ho5Z7SWsYxz5WWT4",
  authDomain: "excelpainter-85f02.firebaseapp.com",
  projectId: "excelpainter-85f02",
  storageBucket: "excelpainter-85f02.appspot.com",
  messagingSenderId: "698684582437",
  appId: "1:698684582437:web:febde42d8a0cfec2476dcc",
  measurementId: "G-R8BE3YDYQP"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);

// Create a writable store for the user
export const user = writable<User | null>(null);

// Get the auth instance
const auth = getAuth(app);

// Function to sign in anonymously
export const signInAnon = async () => {
  try {
    await signInAnonymously(auth);
  } catch (error) {
    console.error('Error signing in anonymously:', error);
  }
};

// Set up an auth state listener
onAuthStateChanged(auth, (firebaseUser) => {
  user.set(firebaseUser);
});