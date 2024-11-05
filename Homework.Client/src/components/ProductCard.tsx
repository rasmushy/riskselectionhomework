import React from 'react';
import { Product } from '../interfaces/product';

interface ProductCardProps {
    product: Product;
}

/**
 * A card component that displays product details including title, brand, and price.
 * @param product - The product data to display.
 * @returns {JSX.Element} The product card component.
 */
const ProductCard: React.FC<ProductCardProps> = ({ product }) => (
    <div className="product-card">
        <h3 className="product-title">{product.title}</h3>
        <div className="product-details">
            {product.brand && <span className="product-brand">{product.brand}</span>}
            <span className="product-price">{product.price} €</span>
        </div>
    </div>
);

export default ProductCard;
