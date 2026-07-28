import { Injectable, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

declare global {
  interface Window {
    gtag?: (...args: unknown[]) => void;
  }
}

@Injectable({ providedIn: 'root' })
export class AnalyticsService {
  private readonly isBrowser = isPlatformBrowser(inject(PLATFORM_ID));

  trackPageView(path: string, title: string) {
    if (!this.isBrowser || typeof window.gtag !== 'function') return;

    window.gtag('event', 'page_view', {
      page_path: path,
      page_title: title,
      page_location: window.location.href,
    });
  }

  trackEvent(name: string, params?: Record<string, unknown>) {
    if (!this.isBrowser || typeof window.gtag !== 'function') return;

    window.gtag('event', name, params);
  }
}
