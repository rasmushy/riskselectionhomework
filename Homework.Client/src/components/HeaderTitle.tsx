import React from 'react';

interface HeaderTitleProps {
    title: string;
}

/**
 * A header title component for displaying main page titles.
 * @param title - The title text to display.
 * @returns {JSX.Element} The header title component.
 */
const HeaderTitle: React.FC<HeaderTitleProps> = ({ title }) => (
    <h2 className="header-title">{title}</h2>
);

export default HeaderTitle;
