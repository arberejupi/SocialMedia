import React, { useEffect, useState } from 'react';
import { BrowserRouter as Router, Switch, Route, Link, Redirect } from 'react-router-dom';
import './App.css';
import Login from './Components/Login';
import Post from './Components/Post';
import Header from './Components/Header';

const PrivateRoute = ({ component: Component, isLoggedIn, userId, token, ...rest }) => (
  <Route
    {...rest}
    render={props => isLoggedIn ? <Component userId={userId} token={token} {...props} /> : <Redirect to="/" />}
  />
);

const App = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [userRole, setUserRole] = useState('');
  const [username, setUsername] = useState('');
  const [token, setToken] = useState('');
  const [userId, setUserId] = useState('');

  const handleLogin = (role, token, userId) => {
    setIsLoggedIn(true);
    setUserRole(role);
    setToken(token);
    setUserId(userId);
    setUsername('');
    const userData = { isLoggedIn: true, userRole: role, token, userId };
    sessionStorage.setItem('userData', JSON.stringify(userData));
  };

  const handleLogout = () => {
    setIsLoggedIn(false);
    setUserRole('');
    setUserId('');
    sessionStorage.removeItem('userData');
  };

  useEffect(() => {
    const storedUserData = sessionStorage.getItem('userData');
    if (storedUserData) {
      const { isLoggedIn, userRole, token, userId } = JSON.parse(storedUserData);
      setIsLoggedIn(isLoggedIn);
      setUserRole(userRole);
      setToken(token);
      setUserId(userId);
    }
  }, []);

  return (
    <div className="App">
      <Router>
        {isLoggedIn && <Header handleLogout={handleLogout} />}
        <Switch>
          <Route exact path="/">
            {isLoggedIn ? <Redirect to="/posts" /> : <Login handleLogin={handleLogin} />}
          </Route>
          <PrivateRoute
            path="/posts"
            component={Post}
            isLoggedIn={isLoggedIn}
            userId={userId}
            token={token}
          />
        </Switch>
      </Router>
    </div>
  );
};

export default App;
