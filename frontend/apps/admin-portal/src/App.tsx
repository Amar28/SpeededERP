import { Navigate, Route, Routes } from 'react-router-dom';

import { AuthProvider, useAuth } from './auth/AuthContext';
import { AppLayout } from './components/AppLayout';
import { FormBuilderDemoPage } from './pages/admin/FormBuilderDemoPage';
import { DevisPage } from './pages/crm/DevisPage';
import { RegisterClientPage } from './pages/crm/RegisterClientPage';
import { DashboardPage } from './pages/DashboardPage';
import { LoginPage } from './pages/LoginPage';

function RequireAuth({ children }: { children: React.ReactElement }) {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? children : <Navigate to="/login" replace />;
}

export function App() {
  return (
    <AuthProvider>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route
          element={
            <RequireAuth>
              <AppLayout />
            </RequireAuth>
          }
        >
          <Route index element={<DashboardPage />} />
          <Route path="crm/clients" element={<RegisterClientPage />} />
          <Route path="crm/devis" element={<DevisPage />} />
          <Route path="admin/forms" element={<FormBuilderDemoPage />} />
        </Route>
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </AuthProvider>
  );
}
