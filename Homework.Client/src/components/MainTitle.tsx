import React from 'react';

interface MainTitleProps {
    title: string;
}

/**
 * A header title component for displaying main page titles.
 * @param title - The title text to display.
 * @returns {JSX.Element} The header title component.
 */
const MainTitle: React.FC<MainTitleProps> = ({ title }) => (
    <h2 className="main-title">{title}</h2>
);

export default MainTitle;
