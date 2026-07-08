import DarkModeIcon from '@mui/icons-material/DarkModeOutlined';
import LightModeIcon from '@mui/icons-material/LightModeOutlined';
import LogoutIcon from '@mui/icons-material/LogoutOutlined';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Drawer from '@mui/material/Drawer';
import IconButton from '@mui/material/IconButton';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import { useThemeMode } from '@speedederp/ui-kit';
import { useTranslation } from 'react-i18next';
import { NavLink, Outlet } from 'react-router-dom';

import { useAuth } from '../auth/AuthContext';

const DRAWER_WIDTH = 240;

export function AppLayout() {
  const { t } = useTranslation();
  const { mode, toggleMode } = useThemeMode();
  const { logout } = useAuth();

  const navItems = [
    { to: '/', label: t('nav.dashboard') },
    { to: '/crm/clients', label: t('nav.clients') },
    { to: '/crm/devis', label: t('crm.devis.title') },
    { to: '/admin/forms', label: t('nav.forms') },
  ];

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar sx={{ gap: 1 }}>
          <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
            {t('app.adminPortalName')}
          </Typography>
          <IconButton color="inherit" onClick={toggleMode} aria-label="toggle theme">
            {mode === 'dark' ? <LightModeIcon /> : <DarkModeIcon />}
          </IconButton>
          <IconButton color="inherit" onClick={logout} aria-label={t('nav.logout')}>
            <LogoutIcon />
          </IconButton>
        </Toolbar>
      </AppBar>
      <Drawer
        variant="permanent"
        sx={{
          width: DRAWER_WIDTH,
          flexShrink: 0,
          [`& .MuiDrawer-paper`]: { width: DRAWER_WIDTH, boxSizing: 'border-box' },
        }}
      >
        <Toolbar />
        <List>
          {navItems.map((item) => (
            <ListItemButton key={item.to} component={NavLink} to={item.to} end={item.to === '/'}>
              <ListItemText primary={item.label} />
            </ListItemButton>
          ))}
        </List>
      </Drawer>
      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  );
}
