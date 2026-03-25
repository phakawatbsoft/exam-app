import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class Quiz {
  private readonly http = inject(HttpClient);

  private apiUrl = 'http://localhost:5076/api';

  getQuestions() {
    return this.http.get<any[]>(`${this.apiUrl}/Questions`);
  }

  createQuestion(data: any) {
    return this.http.post(`${this.apiUrl}/Questions`, data);
  }

  submitResult(data: any) {
    return this.http.post(`${this.apiUrl}/Results`, data);
  }

  getResults() {
    return this.http.get<any[]>(`${this.apiUrl}/Results`);
  }

  getResultById(id: number) {
    return this.http.get<any>(`${this.apiUrl}/Results/${id}`);
  }
}
