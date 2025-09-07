import { StrictMode, useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import { Login } from './pages/Login'

function App() {
  const [count, setCount] = useState(0)

  return (
    <StrictMode>
        <Login />
    </StrictMode>
  )
}

export default App
