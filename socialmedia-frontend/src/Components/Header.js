import React from 'react';
import { Link } from 'react-router-dom';

const Header = ({ handleLogout }) => {
  return (
    <div className="header">
      <div className="header-right">
        <Link className="profile" to="/profile">
          Profile
        </Link>
        <button className="logOutButton" onClick={handleLogout}>
          Logout
        </button>
      </div>
    </div>
  );
};

export default Header;
