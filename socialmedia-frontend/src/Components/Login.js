import React, { useState } from 'react';
import '../CssDesigns/LoginRegister.css';

const Login = ({ handleLogin }) => {
  const [userId, setUserId] = useState('');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [showRegisterForm, setShowRegisterForm] = useState(false);
  const [registerUsername, setRegisterUsername] = useState('');
  const [name, setName] = useState('');
  const [surname, setSurname] = useState('');
  const [email, setEmail] = useState('');
  const [registerPassword, setRegisterPassword] = useState('');

  const handleToggleForm = () => {
    setShowRegisterForm(!showRegisterForm);
  };

  const handleRegisterSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch('https://localhost:7069/api/Auth/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          username: registerUsername,
          name,
          surname,
          email,
          password: registerPassword,
          role: 'User',
        }),
      });

      if (response.ok) {
        const jsonResponse = await response.json();
        const { token, role, userId } = jsonResponse;

        localStorage.setItem('token', token);
        localStorage.setItem('userId', userId);
        localStorage.setItem('role', role);

        handleLogin(role, token, userId);
      } else {
        const errorMessage = await response.text();
        setError(errorMessage);
        console.log('Registration failed:', errorMessage);
      }
    } catch (error) {
      console.log('Error occurred during registration:', error);
    }
  };

  const handleLoginSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch('https://localhost:7069/api/Auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username, password }),
      });

      if (response.ok) {
        const jsonResponse = await response.json();
        const { token, role, userId } = jsonResponse;

        localStorage.setItem('token', token);
        localStorage.setItem('userId', userId);
        localStorage.setItem('role', role);
        
        handleLogin(role, token, userId);
      } else {
        const errorMessage = await response.text();
        setError(errorMessage);
        console.log('Login failed:', errorMessage);
      }
    } catch (error) {
      console.log('Error occurred during login:', error);
    }
  };

  return (
    <div className="login-container">
      {!showRegisterForm ? (
        <>
          <h2>Login</h2>
          <form onSubmit={handleLoginSubmit}>
            <div className="form-group">
              <label htmlFor="username">Username:</label>
              <input
                type="text"
                id="username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="password">Password:</label>
              <input
                type="password"
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>
            {error && <p className="error">{error}</p>}
            <button type="submit" className="btn-primary">
              Login
            </button>
            <p>
              Don't have an account?{' '}
              <button type="button" className="btn-link" onClick={handleToggleForm}>
                Register
              </button>
            </p>
          </form>
        </>
      ) : (
        <>
          <h2>Register</h2>
          <form onSubmit={handleRegisterSubmit}>
            <div className="form-group">
              <label htmlFor="registerUsername">Username:</label>
              <input
                type="text"
                id="registerUsername"
                value={registerUsername}
                onChange={(e) => setRegisterUsername(e.target.value)}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="name">Name:</label>
              <input
                type="text"
                id="name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="surname">Surname:</label>
              <input
                type="text"
                id="surname"
                value={surname}
                onChange={(e) => setSurname(e.target.value)}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="email">Email:</label>
              <input
                type="email"
                id="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="registerPassword">Password:</label>
              <input
                type="password"
                id="registerPassword"
                value={registerPassword}
                onChange={(e) => setRegisterPassword(e.target.value)}
                required
              />
            </div>
            <button type="submit" className="btn-primary">
              Register
            </button>
            <p>
              Already have an account?{' '}
              <button type="button" className="btn-link" onClick={handleToggleForm}>
                Login
              </button>
            </p>
          </form>
        </>
      )}
    </div>
  );
};

export default Login;
