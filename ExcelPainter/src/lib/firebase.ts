import { initializeApp, getApps, type FirebaseOptions } from 'firebase/app';
import { getFunctions, connectFunctionsEmulator, type Functions } from 'firebase/functions';


const firebaseConfig: FirebaseOptions = {
  // Your Firebase config object
  // You can find this in your Firebase project settings
  apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
  authDomain: import.meta.env.VITE_FIREBASE_AUTH_DOMAIN,
  projectId: import.meta.env.VITE_FIREBASE_PROJECT_ID,
  storageBucket: import.meta.env.VITE_FIREBASE_STORAGE_BUCKET,
  messagingSenderId: import.meta.env.VITE_FIREBASE_MESSAGING_SENDER_ID,
  appId: import.meta.env.VITE_FIREBASE_APP_ID
};

function getFirebaseApp() {
  if (getApps().length === 0) {
    return initializeApp(firebaseConfig);
  } else {
    return getApps()[0];
  }
}

const app = getFirebaseApp();
const functions: Functions = getFunctions(app);

if (typeof window !== 'undefined' && import.meta.env.VITE_FIREBASE_USE_EMULATOR === 'true') {
  connectFunctionsEmulator(functions, 'localhost', 5001);
}

export { functions };