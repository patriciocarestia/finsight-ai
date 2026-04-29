import { Component, inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent],
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  private readonly http = inject(HttpClient);

  ngOnInit(): void {
    this.http.get(`${environment.apiUrl}/health`).subscribe();
  }
}
