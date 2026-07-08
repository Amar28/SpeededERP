import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { alpha, useTheme } from '@mui/material/styles';
import type { ReactNode } from 'react';
import { Area, AreaChart, ResponsiveContainer } from 'recharts';

export interface StatCardTrendPoint {
  value: number;
}

export interface StatCardProps {
  label: string;
  value: string | number;
  icon?: ReactNode;
  /** Optional small delta/context line, e.g. "+12% vs last month". */
  helperText?: string;
  /** Optional sparkline data (section 14 "Tableaux de bord & widgets"). */
  trend?: StatCardTrendPoint[];
  color?: 'primary' | 'secondary' | 'success' | 'warning' | 'error' | 'info';
}

/** KPI tile used on dashboard widgets (architecture section 14). */
export function StatCard({
  label,
  value,
  icon,
  helperText,
  trend,
  color = 'primary',
}: StatCardProps) {
  const theme = useTheme();
  const sparklineColor = theme.palette[color].main;
  const gradientId = `statcard-gradient-${label.replace(/\s+/g, '-')}`;

  return (
    <Card variant="outlined" sx={{ height: '100%' }}>
      <CardContent>
        <Stack direction="row" alignItems="flex-start" justifyContent="space-between">
          <Stack spacing={0.5}>
            <Typography variant="body2" color="text.secondary">
              {label}
            </Typography>
            <Typography variant="h4" sx={{ fontWeight: 700 }}>
              {value}
            </Typography>
            {helperText ? (
              <Typography variant="caption" color="text.secondary">
                {helperText}
              </Typography>
            ) : null}
          </Stack>
          {icon ? (
            <Stack
              sx={{
                width: 40,
                height: 40,
                borderRadius: '50%',
                alignItems: 'center',
                justifyContent: 'center',
                color: `${color}.main`,
                bgcolor: (theme) => alpha(theme.palette[color].main, 0.12),
              }}
            >
              {icon}
            </Stack>
          ) : null}
        </Stack>
        {trend && trend.length > 1 ? (
          <Stack sx={{ height: 40, mt: 1.5 }}>
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart data={trend}>
                <defs>
                  <linearGradient id={gradientId} x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor={sparklineColor} stopOpacity={0.4} />
                    <stop offset="95%" stopColor={sparklineColor} stopOpacity={0} />
                  </linearGradient>
                </defs>
                <Area
                  type="monotone"
                  dataKey="value"
                  stroke={sparklineColor}
                  strokeWidth={2}
                  fill={`url(#${gradientId})`}
                  isAnimationActive={false}
                />
              </AreaChart>
            </ResponsiveContainer>
          </Stack>
        ) : null}
      </CardContent>
    </Card>
  );
}
