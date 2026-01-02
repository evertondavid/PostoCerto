# Architecture Decision Records (ADR)

Este diretório contém os registros de decisões arquiteturais tomadas no projeto PostoCerto.

## Template ADR

Use o seguinte template ao criar um novo ADR:

```markdown
# [número]. [Título da decisão]

Data: YYYY-MM-DD

## Status

[Proposto | Aceito | Deprecado | Substituído]

## Contexto

Descreva o contexto e o problema que levou à necessidade desta decisão.

## Decisão

Descreva a decisão tomada e a solução escolhida.

## Consequências

### Positivas
- Lista dos impactos positivos

### Negativas
- Lista dos trade-offs e impactos negativos

## Alternativas Consideradas

Liste outras alternativas que foram consideradas e por que não foram escolhidas.

## Referências

- Links para documentação relevante
- Discussões relacionadas
```

## ADRs Existentes

1. [ADR-001: Uso de Microserviços com gRPC](001-microservices-grpc.md) _(exemplo)_
2. [ADR-002: Clean Architecture por Serviço](002-clean-architecture.md) _(exemplo)_

## Como adicionar um novo ADR

1. Crie um novo arquivo seguindo o padrão: `XXX-titulo-kebab-case.md`
2. Use o próximo número sequencial disponível
3. Preencha todas as seções do template
4. Adicione uma entrada neste README
5. Commit com a mensagem: `docs: add ADR-XXX about [assunto]`
