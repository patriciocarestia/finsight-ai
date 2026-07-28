import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter, map } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { SeoService } from './core/services/seo.service';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent],
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly seo = inject(SeoService);
  private readonly destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.http.get(`${environment.apiUrl}/health`).subscribe();

    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        map(() => {
          let route = this.activatedRoute;
          while (route.firstChild) route = route.firstChild;
          return route.snapshot;
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((snapshot) => {
        const title = (snapshot.data['title'] as string) ?? 'FinSight AI';
        const description =
          (snapshot.data['description'] as string) ??
          'Cotización del dólar y criptomonedas en Argentina hoy, en vivo.';
        this.seo.update({ title, description, path: this.router.url });
      });
  }
}
