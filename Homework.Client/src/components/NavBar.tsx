import React, { useState } from 'react';

/**
 * A navigation bar with links and a hamburger menu for smaller screens.
 * @returns {JSX.Element} The navbar component.
 */
const NavBar: React.FC = () => {
    const [menuOpen, setMenuOpen] = useState(false);

    const toggleMenu = () => {
        setMenuOpen(!menuOpen);
    };

    return (
        <nav className="navbar">
            <div className="navbar-content">
                <div className="navbar-brand-container">
                    <span className="navbar-brand">HomeworkMvc</span>
                    <div className={`navbar-links ${menuOpen ? 'open' : ''}`}>
                        <a href="/" className="navbar-link">Home</a>
                    </div>
                </div>
                <div className="hamburger-menu" onClick={toggleMenu}>
                    <span className="hamburger-bar"></span>
                    <span className="hamburger-bar"></span>
                    <span className="hamburger-bar"></span>
                </div>
            </div>
        </nav>
    );
};

export default NavBar;
