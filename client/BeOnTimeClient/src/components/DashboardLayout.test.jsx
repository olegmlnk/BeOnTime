import { render, screen } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { DashboardLayout } from './DashboardLayout';
import { AuthContext } from '../context/AuthContext';
import { ThemeContext } from '../context/ThemeContext';
import { BrowserRouter } from 'react-router-dom';

describe('DashboardLayout Component', () => {
    const renderWithProviders = (children) => {
        return render(
            <BrowserRouter>
                <ThemeContext.Provider value={{ theme: 'light', toggleTheme: vi.fn() }}>
                    <AuthContext.Provider value={{ user: { id: '1', name: 'Test' }, logout: vi.fn(), isLoading: false }}>
                        <DashboardLayout>{children}</DashboardLayout>
                    </AuthContext.Provider>
                </ThemeContext.Provider>
            </BrowserRouter>
        );
    };

    it('повинен рендерити Sidebar', () => {
        renderWithProviders(<div>Content</div>);
        expect(screen.getByText('BeOnTime')).toBeInTheDocument();
    });

    it('повинен рендерити дочірній контент', () => {
        renderWithProviders(<div>Test Content</div>);
        expect(screen.getByText('Test Content')).toBeInTheDocument();
    });

    it('повинен мати контейнер dashboard-container', () => {
        renderWithProviders(<div>Content</div>);
        expect(document.querySelector('.dashboard-container')).toBeTruthy();
    });

    it('повинен мати main елемент dashboard-main', () => {
        renderWithProviders(<div>Content</div>);
        expect(document.querySelector('.dashboard-main')).toBeTruthy();
    });
});
