import { render, screen } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { PrivateRoute } from './PrivateRoute';
import { AuthContext } from '../context/AuthContext';
import { MemoryRouter } from 'react-router-dom';

describe('PrivateRoute Component', () => {
    it('повинен рендерити дочірній компонент, якщо користувач авторизований', () => {
        render(
            <MemoryRouter>
                <AuthContext.Provider value={{ user: { id: '1', name: 'Test' }, isLoading: false }}>
                    <PrivateRoute>
                        <div>Protected Content</div>
                    </PrivateRoute>
                </AuthContext.Provider>
            </MemoryRouter>
        );

        expect(screen.getByText('Protected Content')).toBeInTheDocument();
    });

    it('повинен перенаправляти на /login, якщо користувач не авторизований', () => {
        render(
            <MemoryRouter initialEntries={['/dashboard']}>
                <AuthContext.Provider value={{ user: null, isLoading: false }}>
                    <PrivateRoute>
                        <div>Protected Content</div>
                    </PrivateRoute>
                </AuthContext.Provider>
            </MemoryRouter>
        );

        expect(screen.queryByText('Protected Content')).not.toBeInTheDocument();
    });

    it('повинен показувати Loading під час завантаження', () => {
        render(
            <MemoryRouter>
                <AuthContext.Provider value={{ user: null, isLoading: true }}>
                    <PrivateRoute>
                        <div>Protected Content</div>
                    </PrivateRoute>
                </AuthContext.Provider>
            </MemoryRouter>
        );

        expect(screen.getByText('Loading...')).toBeInTheDocument();
        expect(screen.queryByText('Protected Content')).not.toBeInTheDocument();
    });
});
